// src/Store/baseState.ts
export interface BaseState {
    isLoading: boolean;
    error: string | null;
  }
  
  export const BASE_INITIAL_STATE: BaseState = {
    isLoading: false,
    error: null
  };