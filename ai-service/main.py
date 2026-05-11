from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List, Optional
from datetime import datetime, timezone

from config import settings
from detector import OccupancyDetector
from client import SmartParkApiClient
from schemas import AnalyzeRequest, BatchAnalyzeRequest, BatchAnalyzeResult, SpaceOccupancyResult, BatchAnalyzeResponse

app = FastAPI(title="SmartPark AI Service", version="1.0.0")

detector = OccupancyDetector()
api_client = SmartParkApiClient(settings.net_api_url, settings.net_api_key)

@app.get("/health")
def health():
    return {
        "status": "ok",
        "service": "smartpark-ai",
        "time": datetime.now(timezone.utc).isoformat()
    }

@app.post("/analyze", response_model=BatchAnalyzeResult)
def analyze(request: AnalyzeRequest):
    try:
        frame = detector.load_image(request.image_url)
        threshold = request.threshold if request.threshold is not None else settings.default_threshold
        source = request.source or settings.source_name

        raw_results = detector.analyze_batch(
            frame=frame,
            spaces=[space.model_dump() for space in request.spaces],
            threshold=threshold
        )

        results: List[SpaceOccupancyResult] = []
        for item in raw_results:
            api_client.send_space_status(
                space_id=item["id"],
                status=item["status"],
                confidence=item["confidence"],
                source=source
            )

            results.append(SpaceOccupancyResult(
                id = item["id"],
                status = item["status"],
                confidence = item["confidence"],
                detected_at = datetime.now(timezone.utc),
                source=source
            ))

        return BatchAnalyzeResponse(
            camera_image=request.image_url,
            results=results
        )

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


@app.post("/analyze-local")
def analyze_local(request: AnalyzeRequest):

    try:
        frame = detector.load_image(request.image_url)
        threshold = request.threshold if request.threshold is not None else settings.default_threshold
        source = request.source or settings.source_name

        raw_results = detector.analyze_batch(
            frame=frame,
            spaces=[space.model_dump() for space in request.spaces],
            threshold=threshold
        )

        return {
            "camera_image": request.image_url,
            "source": source,
            "results": [
                {
                    "id": item["id"],
                    "status": item["status"],
                    "confidence": item["confidence"],
                    "detected_at": datetime.now(timezone.utc).isoformat()
                }
                for item in raw_results
            ]
        }

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/analyze-batch-and-send")
def analyze_batch_and_send(request: BatchAnalyzeRequest):
    try:
        frame = detector.load_image(request.image_path)
        threshold = request.threshold if request.threshold is not None else settings.default_threshold
        source = request.source or settings.source_name

        raw_results = detector.analyze_batch(
            frame=frame,
            spaces=[space.model_dump() for space in request.spaces],
            threshold=threshold
        )

        detections = []
        for item in raw_results:
            detections.append({
                "id": item["id"],
                "status": item["status"],
                "confidence": item["confidence"]
            })

        api_client.send_batch(detections=detections, source=source)

        return {
            "camera_image": request.image_path,
            "sent": True,
            "count": len(detections),
            "detections": detections
        }

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))