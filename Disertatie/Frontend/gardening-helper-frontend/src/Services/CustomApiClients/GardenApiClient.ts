// src/Services/GardenService.ts

import { createApiClient } from "../ApiClient";

const GardenApiClient = createApiClient<any>('Garden');

export default GardenApiClient;