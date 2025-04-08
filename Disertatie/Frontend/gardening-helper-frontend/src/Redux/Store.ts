// src/Redux/Store.ts
import { configureStore } from '@reduxjs/toolkit';
import plantReducer from "../Redux/Plant/PlantSlice";

const store = configureStore({
  reducer: {
    plants: plantReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export default store;