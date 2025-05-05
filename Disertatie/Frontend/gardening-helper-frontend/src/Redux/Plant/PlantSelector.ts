// src/Redux/Plant/PlantSelector.ts
import { createSelector } from '@reduxjs/toolkit';
import { RootState } from "../Store";

// Select all plants
export const selectAllPlants = (state: RootState) => state.plants.plants;

export const selectPlantCards = (state: RootState) => state.plants.plantCards;

// Select a specific plant by id
export const selectPlantById = (id: number) => (state: RootState) => 
  state.plants.plants.find(plant => plant.id === id);

// Select a specific plant by name
export const selectPlantByName = (name: string) => (state: RootState) => 
  state.plants.plants.find(plant => plant.name === name);

// Select the currently selected plant
export const selectSelectedPlant = (state: RootState) => state.plants.selectedPlant;

// Select loading state
export const selectPlantsLoading = (state: RootState) => state.plants.isLoading;

// Select error state
export const selectPlantsError = (state: RootState) => state.plants.error;

// Select all plant details
export const selectAllPlantDetails = (state: RootState) => state.plants.plantDetails;

// Select plant details by plant id
export const selectPlantDetailsById = (plantId: number) => (state: RootState) => 
  state.plants.plantDetails[plantId];

// Memoized selector to check if plant details are loaded for a specific plant
export const selectIsPlantDetailsLoaded = (plantId: number) => 
  createSelector(
    selectAllPlantDetails,
    (plantDetails) => !!plantDetails[plantId]
  );