// src/Components/Auth/Register.tsx
import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { RegisterRequestDTO } from '../../Models/API/DTOs/Auto/Request/registerRequestDTO';
import { useAppDispatch, useAppSelector } from '../../Redux/Hooks/ReduxHooks';
import { selectIsAuthenticated, selectUserError, selectUserLoading } from '../../Redux/User/UserSelector';
import { clearErrors } from '../../Redux/User/UserSlice';
import { registerUser } from '../../Redux/User/UserThunk';
import '../Auth/Auth.css';

const Register: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [passwordError, setPasswordError] = useState('');
  
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  
  const isAuthenticated = useAppSelector(selectIsAuthenticated);
  const loading = useAppSelector(selectUserLoading);
  const error = useAppSelector(selectUserError);
  
  useEffect(() => {
    // Clear any previous errors
    dispatch(clearErrors());
    
    // Redirect if already authenticated
    if (isAuthenticated) {
      navigate('/');
    }
  }, [dispatch, isAuthenticated, navigate]);
  
  const validateForm = () => {
    setPasswordError('');
    
    if (password !== confirmPassword) {
      setPasswordError('Passwords do not match');
      return false;
    }
    
    if (password.length < 6) {
      setPasswordError('Password must be at least 6 characters');
      return false;
    }
    
    return true;
  };
  
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }
    
    const userData: RegisterRequestDTO = {
      email: email,
      password: password,
      role: 'User' // Hardcoded as 'User'
    };
    
    try {
      await dispatch(registerUser(userData));
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
            <div className="card-header auth-header-register">
              <h4 className="mb-0">Join Gardening Helper</h4>
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
                    placeholder="Create a password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                  />
                </div>
                
                <div className="mb-4">
                  <label htmlFor="confirmPassword" className="form-label">Confirm Password</label>
                  <input
                    type="password"
                    className="form-control auth-input"
                    id="confirmPassword"
                    placeholder="Confirm your password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    required
                  />
                  {passwordError && (
                    <div className="form-text text-danger">{passwordError}</div>
                  )}
                </div>
                
                <button 
                  type="submit" 
                  className="btn auth-submit-btn-register w-100"
                  disabled={loading}
                >
                  {loading ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                      Creating Account...
                    </>
                  ) : 'Register'}
                </button>
              </form>
              
              <div className="mt-4 text-center">
                <p className="auth-link-text">
                  Already have an account? <Link to="/login" className="auth-link">Login</Link>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Register;