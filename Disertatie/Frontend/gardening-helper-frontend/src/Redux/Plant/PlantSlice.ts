// src/Redux/Plant/plantSlice.ts
import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { PlantDTO } from '../../Models/API/DTOs/Auto/Response/plantDTO';
import { PLANTS_INITIAL_STATE } from './PlantState';
import { createPlant, deletePlant, fetchAllPlants, fetchPlant, updatePlant } from './PlantThunk';

const plantSlice = createSlice({
  name: 'plants',
  initialState: PLANTS_INITIAL_STATE,
  reducers: {
    resetPlantsState: () => PLANTS_INITIAL_STATE,
    setPlantsLoading: (state, action: PayloadAction<boolean>) => {
      state.isLoading = action.payload;
    },
    setPlantsError: (state, action: PayloadAction<string | null>) => {
      state.error = action.payload;
    },
    setSelectedPlant: (state, action: PayloadAction<PlantDTO | null>) => {
      state.selectedPlant = action.payload;
    },
    updatePlantInList: (state, action: PayloadAction<PlantDTO>) => {
      const index = state.plants.findIndex(p => p.id === action.payload.id);
      if (index !== -1) {
        state.plants[index] = action.payload;
      }
    }
  },
  extraReducers: (builder) => {
    // Handle fetch single plant
    builder.addCase(fetchPlant.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(fetchPlant.fulfilled, (state, action) => {
      state.selectedPlant = action.payload;
      state.isLoading = false;
      state.error = null;
    });
    builder.addCase(fetchPlant.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string || 'Failed to fetch plant';
    });

    // Handle fetch all plants
    builder.addCase(fetchAllPlants.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(fetchAllPlants.fulfilled, (state, action) => {
      state.plants = action.payload;
      state.isLoading = false;
      state.error = null;
    });
    builder.addCase(fetchAllPlants.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string || 'Failed to fetch plants';
    });

    // Handle create plant
    builder.addCase(createPlant.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(createPlant.fulfilled, (state, action) => {
      state.plants.push(action.payload);
      state.isLoading = false;
      state.error = null;
    });
    builder.addCase(createPlant.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string || 'Failed to create plant';
    });

    // Handle update plant
    builder.addCase(updatePlant.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(updatePlant.fulfilled, (state, action) => {
      const index = state.plants.findIndex(p => p.id === action.payload.id);
      if (index !== -1) {
        state.plants[index] = action.payload;
      }
      if (state.selectedPlant?.id === action.payload.id) {
        state.selectedPlant = action.payload;
      }
      state.isLoading = false;
      state.error = null;
    });
    builder.addCase(updatePlant.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string || 'Failed to update plant';
    });

    // Handle delete plant
    builder.addCase(deletePlant.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(deletePlant.fulfilled, (state, action) => {
      state.plants = state.plants.filter(p => p.id !== action.meta.arg);
      if (state.selectedPlant?.id === action.meta.arg) {
        state.selectedPlant = null;
      }
      state.isLoading = false;
      state.error = null;
    });
    builder.addCase(deletePlant.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string || 'Failed to delete plant';
    });
  }
});

export const { 
  resetPlantsState, 
  setPlantsLoading, 
  setPlantsError,
  setSelectedPlant,
  updatePlantInList
} = plantSlice.actions;

export default plantSlice.reducer;