import { NavLink, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../Redux/Hooks/ReduxHooks';
import { selectIsAuthenticated, selectUserRole } from '../../Redux/User/UserSelector';
import { logout } from '../../Redux/User/UserSlice';
import './NavBar.css';

function NavBar() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const isAuthenticated = useAppSelector(selectIsAuthenticated);
  const userRole = useAppSelector(selectUserRole);
  
  const handleLogout = () => {
    dispatch(logout());
    navigate('/login');
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark custom-navbar">
      <div className="container">
        <NavLink className="navbar-brand text-green" to="/">Gardening Helper</NavLink>
        <button 
          className="navbar-toggler" 
          type="button" 
          data-bs-toggle="collapse" 
          data-bs-target="#navbarNav"
        >
          <span className="navbar-toggler-icon"></span>
        </button>
        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav ms-auto">
            {isAuthenticated ? (
              <>
                <li className="nav-item">
                  <NavLink className="nav-link" to="/">My Garden</NavLink>
                </li>
                <li className="nav-item">
                  <NavLink className="nav-link" to="/plants">Plants</NavLink>
                </li>
                {userRole === 'Admin' && (
                  <li className="nav-item">
                    <NavLink className="nav-link" to="/admin">Admin</NavLink>
                  </li>
                )}
                <li className="nav-item">
                  <button 
                    className="btn btn-outline-light nav-link"
                    onClick={handleLogout}
                    style={{ backgroundColor: 'transparent', border: 'none' }}
                  >
                    Logout
                  </button>
                </li>
              </>
            ) : (
              <>
                <li className="nav-item">
                  <NavLink className="nav-link" to="/login">Login</NavLink>
                </li>
                <li className="nav-item">
                  <NavLink className="nav-link" to="/register">Register</NavLink>
                </li>
              </>
            )}
          </ul>
        </div>
      </div>
    </nav>
  );
}

export default NavBar;