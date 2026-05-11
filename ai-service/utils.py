from datetime import datetime, timezone
from typing import List
import numpy as np

def utc_now() -> datetime:
    return datetime.now(timezone.utc)

def polygon_to_np(polygon: List[List[int]]) -> np.ndarray:
    return np.array(
        polygon,
        dtype=np.int32
    )