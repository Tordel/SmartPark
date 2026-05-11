from dataclasses import dataclass
import os

@dataclass
class Settings:
    net_api_url: str = os.getenv("NET_API_URL", "https://localhost:5001")
    net_api_key: str = os.getenv("NET_API_KEY", "local-dev")
    source_name: str = os.getenv("AI_SOURCE_NAME", "python-ai")
    default_threshold: float = float(os.getenv("OCCUPANCY_THRESHOLD", "0.08"))

settings = Settings()