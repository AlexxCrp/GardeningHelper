// src/Redux/User/UserThunks.ts
import { LoginRequestDTO } from '../../Models/API/DTOs/Auto/Request/loginRequestDTO';
import { RegisterRequestDTO } from '../../Models/API/DTOs/Auto/Request/registerRequestDTO';
import { AuthService } from '../../Services/AuthService';
import { AppDispatch } from '../Store';
import {
    loginFailure,
    loginStart,
    loginSuccess,
    registerFailure,
    registerStart,
    registerSuccess
} from './UserSlice';

export const loginUser = (credentials: LoginRequestDTO) => async (dispatch: AppDispatch) => {
  try {
    dispatch(loginStart());
    const response = await AuthService.login(credentials);
    dispatch(loginSuccess(response));
    return response;
  } catch (error) {
    dispatch(loginFailure(error instanceof Error ? error.message : 'An unknown error occurred'));
    throw error;
  }
};

export const registerUser = (userData: RegisterRequestDTO) => async (dispatch: AppDispatch) => {
  try {
    dispatch(registerStart());
    const response = await AuthService.register(userData);
    dispatch(registerSuccess(response));
    return response;
  } catch (error) {
    dispatch(registerFailure(error instanceof Error ? error.message : 'An unknown error occurred'));
    throw error;
  }
};