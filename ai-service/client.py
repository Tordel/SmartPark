from datetime import datetime, timezone
import requests

class SmartParkApiClient:
    def __init__(self, base_url: str, api_key: str):
        self.base_url = base_url
        self.api_key = api_key

    def send_space_status(self, space_id: str, status: str, confidence: float, source: str):
        payload = {
            "id": space_id,
            "newStatus": status,
            "detectedAt": datetime.now(timezone.utc).isoformat(),
            "confidence": confidence,
            "source": source
        }

        headers = {
            "X-API-KEY": self.api_key,
            "Content-Type": "application/json"
        }

        url = f"{self.base_url}/api/ai/occupancy"
        response = requests.post(url, json=payload, headers=headers, timeout=10)
        response.raise_for_status()
        return response.status_code

    def send_batch(self, detections: list, source: str):
        payload = {
            "detectedAt": datetime.now(timezone.utc).isoformat(),
            "source": source,
            "detections": detections
        }

        headers = {
            "X-API-KEY": self.api_key,
            "Content-Type": "application/json"
        }

        url = f"{self.base_url}/api/ai/occupancy/batch"
        response = requests.post(url, json=payload, headers=headers, timeout=10)
        response.raise_for_status()
        return response.status_code