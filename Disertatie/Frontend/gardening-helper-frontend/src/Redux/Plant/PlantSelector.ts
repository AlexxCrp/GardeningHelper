// src/Redux/Plant/PlantSelector.ts
import { RootState } from "../Store";

// Select all plants
export const selectAllPlants = (state: RootState) => state.plants.plants;

// Select a specific plant by id
export const selectPlantById = (id: number) => (state: RootState) => 
  state.plants.plants.find(plant => plant.id === id);

// Select the currently selected plant
export const selectSelectedPlant = (state: RootState) => state.plants.selectedPlant;

// Select loading state
export const selectPlantsLoading = (state: RootState) => state.plants.isLoading;

// Select error state
export const selectPlantsError = (state: RootState) => state.plants.error;