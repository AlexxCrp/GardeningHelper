// src/Redux/User/UserSlice.ts
import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { AuthResponseDTO } from '../../Models/API/DTOs/Auto/Response/authResponseDTO';
import { USER_INITIAL_STATE } from './UserState';

const userSlice = createSlice({
  name: 'user',
  initialState: USER_INITIAL_STATE,
  reducers: {
    loginStart: (state) => {
      state.loading = true;
      state.error = null;
    },
    loginSuccess: (state, action: PayloadAction<AuthResponseDTO>) => {
      state.id = action.payload.id;
      state.token = action.payload.token;
      state.role = action.payload.role;
      state.isAuthenticated = true;
      state.loading = false;
      state.error = null;
      
      // Store in localStorage for persistence
      localStorage.setItem('token', action.payload.token);
      localStorage.setItem('role', action.payload.role);
    },
    loginFailure: (state, action: PayloadAction<string>) => {
      state.loading = false;
      state.error = action.payload;
    },
    registerStart: (state) => {
      state.loading = true;
      state.error = null;
    },
    registerSuccess: (state, action: PayloadAction<AuthResponseDTO>) => {
      state.token = action.payload.token;
      state.role = action.payload.role;
      state.isAuthenticated = true;
      state.loading = false;
      state.error = null;
      
      // Store in localStorage for persistence
      localStorage.setItem('token', action.payload.token);
      localStorage.setItem('role', action.payload.role);
    },
    registerFailure: (state, action: PayloadAction<string>) => {
      state.loading = false;
      state.error = action.payload;
    },
    logout: (state) => {
      state.token = null;
      state.role = null;
      state.isAuthenticated = false;
      
      // Remove from localStorage
      localStorage.removeItem('token');
      localStorage.removeItem('role');
    },
    clearErrors: (state) => {
      state.error = null;
    }
  },
});

export const { 
  loginStart, 
  loginSuccess, 
  loginFailure, 
  registerStart, 
  registerSuccess, 
  registerFailure,
  logout,
  clearErrors
} = userSlice.actions;

export default userSlice.reducer;