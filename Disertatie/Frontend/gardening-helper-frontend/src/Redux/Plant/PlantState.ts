// src/Models/State/plantState.ts
import { PlantCardDTO } from "../../Models/API/DTOs/Auto/Response/plantCardDTO";
import { PlantDTO } from "../../Models/API/DTOs/Auto/Response/plantDTO";
import { BASE_INITIAL_STATE, BaseState } from "../Generics/BaseState";

// Update the state to handle multiple plants
export interface PlantsState extends BaseState {
  plants: PlantDTO[];
  plantCards: PlantCardDTO[];
  selectedPlant: PlantDTO | null;
}

export const PLANTS_INITIAL_STATE: PlantsState = {
  plants: [],
  plantCards: [],
  selectedPlant: null,
  ...BASE_INITIAL_STATE
};