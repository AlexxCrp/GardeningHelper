export interface UserState {
    id: string;
    token: string | null;
    role: string | null;
    isAuthenticated: boolean;
    loading: boolean;
    error: string | null;
}
  
export const USER_INITIAL_STATE: UserState = {
    id: "0",
    token: localStorage.getItem('token'),
    role: localStorage.getItem('role'),
    isAuthenticated: !!localStorage.getItem('token'),
    loading: false,
    error: null,
};