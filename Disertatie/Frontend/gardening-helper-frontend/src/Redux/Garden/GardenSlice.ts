import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { GARDEN_INITIAL_STATE, TempGardenPlant } from './GardenState';
import {
  addPlantToGarden,
  batchUpdateGarden,
  createGarden,
  fetchUserGarden,
  removePlantFromGarden,
  updateGarden
} from './GardenThunk';

const gardenSlice = createSlice({
  name: 'garden',
  initialState: GARDEN_INITIAL_STATE,
  reducers: {
    resetGardenState: () => GARDEN_INITIAL_STATE,
    setGardenLoading: (state, action: PayloadAction<boolean>) => {
      state.loading = action.payload;
    },
    setGardenError: (state, action: PayloadAction<string | null>) => {
      state.error = action.payload;
    },
    toggleCreateGardenForm: (state) => {
      state.isCreatingGarden = !state.isCreatingGarden;
    },
    // New reducers for temporary garden changes
    initializeEditMode: (state) => {
      if (state.garden) {
        state.tempGarden = {
          plants: state.garden.gardenPlants.map(plant => ({
            id: plant.id,
            plantId: plant.plantId,
            plantName: plant.plantName,
            positionX: plant.positionX,
            positionY: plant.positionY,
            base64Image: plant.base64Image,
            operation: 'add' // Existing plants are treated as 'add' initially
          })),
          xSize: state.garden.xSize,
          ySize: state.garden.ySize
        };
      }
    },
    cancelEditMode: (state) => {
      state.tempGarden = null;
    },
    updateTempGardenSize: (state, action: PayloadAction<{ xSize: number, ySize: number }>) => {
      if (state.tempGarden) {
        state.tempGarden.xSize = action.payload.xSize;
        state.tempGarden.ySize = action.payload.ySize;
      }
    },
    addTempPlant: (state, action: PayloadAction<Omit<TempGardenPlant, 'operation'>>) => {
      if (state.tempGarden) {
        // Check if the position is already occupied
        const existingPlantIndex = state.tempGarden.plants.findIndex(
          p => p.positionX === action.payload.positionX && p.positionY === action.payload.positionY
        );
        
        // If position is occupied, remove the existing plant
        if (existingPlantIndex !== -1) {
          state.tempGarden.plants.splice(existingPlantIndex, 1);
        }
        
        // Add the new plant
        state.tempGarden.plants.push({
          ...action.payload,
          operation: 'add'
        });
      }
    },
    moveTempPlant: (state, action: PayloadAction<{
      fromX: number,
      fromY: number,
      toX: number,
      toY: number
    }>) => {
      if (state.tempGarden) {
        const { fromX, fromY, toX, toY } = action.payload;
        
        // Find plant at source position
        const plantIndex = state.tempGarden.plants.findIndex(
          p => p.positionX === fromX && p.positionY === fromY && p.operation !== 'remove'
        );
        
        if (plantIndex !== -1) {
          const plant = state.tempGarden.plants[plantIndex];
          
          // Check if destination is already occupied
          const destinationIndex = state.tempGarden.plants.findIndex(
            p => p.positionX === toX && p.positionY === toY && p.operation !== 'remove'
          );
          
          // If destination is occupied, remove that plant
          if (destinationIndex !== -1) {
            state.tempGarden.plants.splice(destinationIndex, 1);
          }
          
          // Set operation based on if it's an existing plant (has an ID)
          // If it has an ID, it's from the database and should be a 'move' operation
          // Otherwise, it's a new plant being moved around before saving ('add' operation)
          const isExistingPlant = !!plant.id;
          
          // Update plant position
          state.tempGarden.plants[plantIndex] = {
            ...plant,
            positionX: toX,
            positionY: toY,
            originalPosition: plant.originalPosition || { x: fromX, y: fromY },
            operation: isExistingPlant ? 'move' : 'add'
          };
        }
      }
    },
    removeTempPlant: (state, action: PayloadAction<{ x: number, y: number }>) => {
      if (state.tempGarden) {
        const plantIndex = state.tempGarden.plants.findIndex(
          p => p.positionX === action.payload.x && p.positionY === action.payload.y && p.operation !== 'remove'
        );
        
        if (plantIndex !== -1) {
          // If this was an existing plant, mark it for removal
          const plant = state.tempGarden.plants[plantIndex];
          if (plant.id) {
            state.tempGarden.plants[plantIndex] = {
              ...plant,
              operation: 'remove'
            };
          } else {
            // If it was a new plant, just remove it from the array
            state.tempGarden.plants.splice(plantIndex, 1);
          }
        }
      }
    }
  },
  extraReducers: (builder) => {
    // fetchUserGarden
    builder.addCase(fetchUserGarden.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(fetchUserGarden.fulfilled, (state, action) => {
      state.garden = action.payload;
      state.loading = false;
    });
    builder.addCase(fetchUserGarden.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload as string || 'Failed to fetch garden';
    });

    // createGarden
    builder.addCase(createGarden.pending, (state) => {
      state.loading = true;
      state.isCreatingGarden = true;
      state.error = null;
    });
    builder.addCase(createGarden.fulfilled, (state, action) => {
      state.garden = action.payload;
      state.loading = false;
      state.isCreatingGarden = false;
    });
    builder.addCase(createGarden.rejected, (state, action) => {
      state.loading = false;
      state.isCreatingGarden = false;
      state.error = action.payload as string || 'Failed to create garden';
    });

    // Add handlers for the batch update thunk
    builder.addCase(batchUpdateGarden.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(batchUpdateGarden.fulfilled, (state, action) => {
      state.garden = action.payload;
      state.tempGarden = null;
      state.loading = false;
    });
    builder.addCase(batchUpdateGarden.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload as string || 'Failed to update garden';
    });

    // Keeping the individual action handlers for backward compatibility
    builder.addCase(addPlantToGarden.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(addPlantToGarden.fulfilled, (state, action) => {
      if (state.garden) {
        state.garden.gardenPlants.push(action.payload);
      }
      state.loading = false;
    });
    builder.addCase(addPlantToGarden.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload as string || 'Failed to add plant to garden';
    });

    builder.addCase(removePlantFromGarden.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(removePlantFromGarden.fulfilled, (state, action) => {
      if (state.garden) {
        state.garden.gardenPlants = state.garden.gardenPlants.filter(p => p.id !== action.payload);
      }
      state.loading = false;
    });
    builder.addCase(removePlantFromGarden.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload as string || 'Failed to remove plant from garden';
    });

    builder.addCase(updateGarden.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(updateGarden.fulfilled, (state, action) => {
      if (state.garden) {
        state.garden = action.payload;
      }
      state.loading = false;
    });
    builder.addCase(updateGarden.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload as string || 'Failed to update garden';
    });
  }
});

export const {
  resetGardenState,
  setGardenLoading,
  setGardenError,
  toggleCreateGardenForm,
  initializeEditMode,
  cancelEditMode,
  updateTempGardenSize,
  addTempPlant,
  moveTempPlant,
  removeTempPlant
} = gardenSlice.actions;

export default gardenSlice.reducer;