import { LoginRequestDTO } from '../Models/API/DTOs/Auto/Request/loginRequestDTO';
import { RegisterRequestDTO } from '../Models/API/DTOs/Auto/Request/registerRequestDTO';
import { AuthResponseDTO } from '../Models/API/DTOs/Auto/Response/authResponseDTO';
import { createApiClient } from './ApiClient';

const AuthApiClient = createApiClient<any>('Identity');

export const AuthService = {
  async login(credentials: LoginRequestDTO) {
    try {
      return await AuthApiClient.customPost<LoginRequestDTO, AuthResponseDTO>('/Login', credentials);
    } catch (error) {
      throw new Error('Login failed');
    }
  },

  async register(credentials: RegisterRequestDTO) {
    try {
      return await AuthApiClient.customPost<RegisterRequestDTO, AuthResponseDTO>('/Register', credentials);
    } catch (error) {
      throw new Error('Registration failed');
    }
  },
};
