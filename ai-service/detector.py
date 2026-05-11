from typing import List, Dict, Any, Optional
import os

import cv2
import numpy as np

from utils import polygon_to_np

class OccupancyDetector:
    def __init__(self):
        self.reference_image_path = os.getenv("EMPTY_REFERENCE_IMAGE", "empty_reference.jpg")
        self.reference_frame = self._load_reference_frame(self.reference_image_path)

    def _load_reference_frame(self, image_path: str) -> Optional[np.ndarray]:
        if not os.path.exists(image_path):
            return None

        frame = cv2.imread(image_path)
        if frame is None:
            return None
        return frame

    def load_image(self, image_path: str):
        frame = cv2.imread(image_path)
        if frame is None:
            raise ValueError(f"Cannot load image: {image_path}")
        return frame

    def load_video_frame(self, video_path: str, frame_index: int = 0):
        cap = cv2.VideoCapture(video_path)
        if not cap.isOpened():
            raise ValueError(f"Cannot open video: {video_path}")

        try:
            cap.set(cv2.CAP_PROP_POS_FRAMES, frame_index)
            ok, frame = cap.read()
            if not ok or frame is None:
                raise ValueError(f"Cannot read frame {frame_index} from video: {video_path}")
            return frame
        finally:
            cap.release()

    def polygon_mask(self, frame_shape, polygon: List[List[int]]):
        mask = np.zeros(frame_shape[:2], dtype=np.uint8)
        pts = polygon_to_np(polygon)
        cv2.fillPoly(mask, [pts], 255)
        return mask


    def overlap_ratio(self, a_mask, b_mask):
        intersection = cv2.bitwise_and(a_mask, b_mask)
        inter_area = cv2.countNonZero(intersection)
        b_area = cv2.countNonZero(b_mask)
        if b_area == 0:
            return 0.0
        return inter_area / b_area

    def _prepare_frame(self, frame: np.ndarray, target_shape: tuple[int, int]) -> np.ndarray:
        if frame.shape[:2] != target_shape:
            return cv2.resize(frame, (target_shape[1], target_shape[0]))
        return frame

    def _normalize_score(self, score: float) -> float:
        return max(0.0, min(1.0, float(score)))

    def _final_label_confidence(self, occupancy_score: float, threshold: float) -> tuple[str, float]:
        occupancy_score = self._normalize_score(occupancy_score)
        threshold = self._normalize_score(threshold)

        if threshold <= 0.0:
            status = "Occupied" if occupancy_score > 0.0 else "Free"
            confidence = occupancy_score if status == "Occupied" else 1.0
            return status, round(float(confidence), 3)

        if occupancy_score >= threshold:
            status = "Occupied"
            if threshold >= 1.0:
                confidence = 1.0
            else:
                confidence = (occupancy_score - threshold) / (1.0 - threshold)
                confidence = 0.5 + 0.5 * confidence
        else:
            status = "Free"
            confidence = 1.0 - (occupancy_score / threshold)
            confidence = 0.5 + 0.5 * confidence

        confidence = self._normalize_score(confidence)
        return status, round(float(confidence), 3)

    def detect_occupancy_score(self, frame: np.ndarray, space_polygon: List[List[int]]) -> float:
        if self.reference_frame is None:
            raise ValueError(
                "EMPTY_REFERENCE_IMAGE is not configured or could not be loaded. "
                "Provide an empty reference image from the same camera angle."
            )

        reference = self._prepare_frame(self.reference_frame, frame.shape[:2])

        frame_gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        ref_gray = cv2.cvtColor(reference, cv2.COLOR_BGR2GRAY)

        space_mask = self.polygon_mask(frame.shape, space_polygon)

        frame_space = cv2.bitwise_and(frame_gray, frame_gray, mask=space_mask)
        ref_space = cv2.bitwise_and(ref_gray, ref_gray, mask=space_mask)

        diff = cv2.absdiff(ref_space, frame_space)

        _, binary = cv2.threshold(diff, 25, 255, cv2.THRESH_BINARY)

        kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (5, 5))
        binary = cv2.morphologyEx(binary, cv2.MORPH_OPEN, kernel, iterations=2)
        binary = cv2.morphologyEx(binary, cv2.MORPH_DILATE, kernel, iterations=2)

        charged_ratio = self.overlap_ratio(binary, space_mask)
        return self._normalize_score(charged_ratio)

    def analyze_space(self, frame, space_polygon: List[List[int]], threshold: float = 0.08) -> Dict[str, Any]:
        occupancy_score = self.detect_occupancy_score(frame, space_polygon)
        status, confidence = self._final_label_confidence(occupancy_score, threshold)

        return {
            "status": status,
            "confidence": confidence
        }

    def analyze_batch(self, frame, spaces: List[Dict[str, Any]], threshold: float = 0.08):
        results = []
        for space in spaces:
            result = self.analyze_space(frame, space["polygon"], threshold)
            results.append({
                "id": space["id"],
                "status": result["status"],
                "confidence": result["confidence"]
            })
        return results