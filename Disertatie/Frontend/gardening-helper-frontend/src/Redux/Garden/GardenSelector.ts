import { RootState } from '../Store';

export const selectGarden = (state: RootState) => state.garden.garden;
export const selectAvailablePlants = (state: RootState) => state.garden.availablePlants;
export const selectGardenLoading = (state: RootState) => state.garden.loading;
export const selectGardenError = (state: RootState) => state.garden.error;
export const selectIsCreatingGarden = (state: RootState) => state.garden.isCreatingGarden;