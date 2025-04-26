import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { GARDEN_INITIAL_STATE } from './GardenState';
import {
  addPlantToGarden,
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

    // addPlantToGarden
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

    // removePlantFromGarden
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

    //update Garden
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
      state.error = action.payload as string || 'Failed to remove plant from garden';
    });
  }
});

export const {
  resetGardenState,
  setGardenLoading,
  setGardenError,
  toggleCreateGardenForm
} = gardenSlice.actions;

export default gardenSlice.reducer;
