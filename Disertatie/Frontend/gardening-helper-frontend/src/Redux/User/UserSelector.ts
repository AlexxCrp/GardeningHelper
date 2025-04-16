import { RootState } from '../Store';

export const selectUser = (state: RootState) => state.user;
export const selectIsAuthenticated = (state: RootState) => state.user.isAuthenticated;
export const selectUserRole = (state: RootState) => state.user.role;
export const selectUserToken = (state: RootState) => state.user.token;
export const selectUserLoading = (state: RootState) => state.user.loading;
export const selectUserError = (state: RootState) => state.user.error;