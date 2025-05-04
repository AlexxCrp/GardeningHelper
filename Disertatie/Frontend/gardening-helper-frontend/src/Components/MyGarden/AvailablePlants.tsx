import React, { useEffect } from "react";
import { selectGardenLoading } from "../../Redux/Garden/GardenSelector";
import { useAppDispatch, useAppSelector } from "../../Redux/Hooks/ReduxHooks";
import { selectAllPlants } from "../../Redux/Plant/PlantSelector";
import { fetchAllPlants } from "../../Redux/Plant/PlantThunk";
import DraggablePlantCard from "./DraggablePlantCard";

const AvailablePlants: React.FC = () => {
  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(fetchAllPlants());
  }, [dispatch]);

  const plants = useAppSelector(selectAllPlants);
  const isLoading = useAppSelector(selectGardenLoading);

  if (isLoading && plants.length === 0) {
    return <div className="text-center p-4">Loading plants...</div>;
  }

  if (plants.length === 0) {
    return <div className="text-center p-4">No plants available</div>;
  }

  return (
    <div className="plant-list-container">
      <h3 className="plant-list-header">Available Plants</h3>
      <div className="plant-grid">
        {plants.map(plant => (
          <DraggablePlantCard
            key={plant.id}
            id={plant.id}
            name={plant.name}
            imageBase64={plant.imageBase64}
          />
        ))}
      </div>
    </div>
  );
};

export default AvailablePlants;