from pydantic import BaseModel
from typing import List, Optional
from datetime import datetime

class ParkingSpaceRegion(BaseModel):
    id: str
    name: str
    polygon: List[List[int]]

class AnalyzeRequest(BaseModel):
    image_url: str
    spaces: List[ParkingSpaceRegion]
    threshold: Optional[float] = None
    source: Optional[str] = None

class BatchAnalyzeRequest(BaseModel):
    image_path: str
    spaces: List[ParkingSpaceRegion]
    threshold: Optional[float] = None
    source: Optional[str] = None

class SpaceOccupancyResult(BaseModel):
    id: str
    status: str
    confidence: float
    detected_at: datetime
    source: Optional[str] = None

class BatchAnalyzeResult(BaseModel):
    id: str
    status: str

class BatchAnalyzeResponse(BaseModel):
    camera_image: str
    results: List[SpaceOccupancyResult]