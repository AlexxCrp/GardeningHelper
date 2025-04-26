import React, { useState } from 'react';
import { createGarden } from '../../Redux/Garden/GardenThunk';
import { useAppDispatch } from '../../Redux/Hooks/ReduxHooks';

interface CreateGardenFormProps {
  onCancel: () => void;
}

const CreateGardenForm: React.FC<CreateGardenFormProps> = ({ onCancel }) => {
  const dispatch = useAppDispatch();
  const [xSize, setXSize] = useState<number>(3);
  const [ySize, setYSize] = useState<number>(3);
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (xSize <= 0 || ySize <= 0) {
      setError('Garden dimensions must be positive numbers');
      return;
    }

    if (xSize > 10 || ySize > 10) {
      setError('Maximum garden size is 10x10');
      return;
    }

    setIsSubmitting(true);
    setError(null);

    try {
      await dispatch(createGarden({ xSize, ySize })).unwrap();
      onCancel(); // Close form on success
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create garden');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="auth-card">
      <div className="auth-header">
        <h3>Create Your Garden</h3>
      </div>
      <div className="auth-body">
        {error && (
          <div className="alert alert-danger mb-3">
            {error}
          </div>
        )}
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label htmlFor="xSize" className="form-label">Width (meters)</label>
            <input
              type="number"
              className="form-control auth-input"
              id="xSize"
              value={xSize}
              onChange={(e) => setXSize(parseInt(e.target.value))}
              min="1"
              max="10"
              required
            />
          </div>
          <div className="mb-3">
            <label htmlFor="ySize" className="form-label">Length (meters)</label>
            <input
              type="number"
              className="form-control auth-input"
              id="ySize"
              value={ySize}
              onChange={(e) => setYSize(parseInt(e.target.value))}
              min="1"
              max="10"
              required
            />
          </div>
          <div className="d-flex justify-content-between mt-4">
            <button
              type="button"
              className="btn btn-secondary"
              onClick={onCancel}
              disabled={isSubmitting}
            >
              Cancel
            </button>
            <button
              type="submit"
              className="btn btn-success"
              disabled={isSubmitting}
            >
              {isSubmitting ? 'Creating...' : 'Create Garden'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CreateGardenForm;
