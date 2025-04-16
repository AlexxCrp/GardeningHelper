// src/Components/Auth/Login.tsx
import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { LoginRequestDTO } from '../../Models/API/DTOs/Auto/Request/loginRequestDTO';
import { useAppDispatch, useAppSelector } from '../../Redux/Hooks/ReduxHooks';
import { selectIsAuthenticated, selectUserError, selectUserLoading } from '../../Redux/User/UserSelector';
import { clearErrors } from '../../Redux/User/UserSlice';
import { loginUser } from '../../Redux/User/UserThunk';
import '../Auth/Auth.css';

const Login: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  
  const isAuthenticated = useAppSelector(selectIsAuthenticated);
  const loading = useAppSelector(selectUserLoading);
  const error = useAppSelector(selectUserError);
  
  useEffect(() => {
    // Clear any previous errors when component mounts
    dispatch(clearErrors());
    
    // Redirect if already authenticated
    if (isAuthenticated) {
      navigate('/');
    }
  }, [dispatch, isAuthenticated, navigate]);
  
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    const credentials: LoginRequestDTO = {
      email: email,
      password: password
    };
    
    try {
      await dispatch(loginUser(credentials));
      navigate('/');
    } catch (err) {
      // Error handling is done in the thunk and stored in Redux state
    }
  };
  
  return (
    <div className="container auth-container">
      <div className="row justify-content-center mt-5">
        <div className="col-md-6">
          <div className="card auth-card">
            <div className="card-header auth-header">
              <h4 className="mb-0">Login to Gardening Helper</h4>
            </div>
            <div className="card-body auth-body">
              {error && (
                <div className="alert alert-danger" role="alert">
                  {error}
                </div>
              )}
              
              <form onSubmit={handleSubmit}>
                <div className="mb-4">
                  <label htmlFor="email" className="form-label">Email address</label>
                  <input
                    type="email"
                    className="form-control auth-input"
                    id="email"
                    placeholder="Enter your email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                  />
                </div>
                
                <div className="mb-4">
                  <label htmlFor="password" className="form-label">Password</label>
                  <input
                    type="password"
                    className="form-control auth-input"
                    id="password"
                    placeholder="Enter your password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                  />
                </div>
                
                <button 
                  type="submit" 
                  className="btn auth-submit-btn w-100"
                  disabled={loading}
                >
                  {loading ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                      Logging in...
                    </>
                  ) : 'Login'}
                </button>
              </form>
              
              <div className="mt-4 text-center">
                <p className="auth-link-text">
                  Don't have an account? <Link to="/register" className="auth-link">Register</Link>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;