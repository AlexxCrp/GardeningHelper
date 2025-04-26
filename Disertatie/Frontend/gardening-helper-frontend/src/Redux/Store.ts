// src/Redux/Store.ts
import { configureStore } from '@reduxjs/toolkit';
import gardenReducer from "./Garden/GardenSlice";
import plantReducer from "./Plant/PlantSlice";
import userReducer from "./User/UserSlice";

const store = configureStore({
  reducer: {
    plants: plantReducer,
    user: userReducer,
    garden: gardenReducer
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export default store;