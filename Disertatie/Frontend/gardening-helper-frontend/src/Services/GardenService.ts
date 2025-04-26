import { AddPlantToGardenRequestDTO } from "../Models/API/DTOs/Auto/Request/addPlantToGardenRequestDTO";
import { CreateGardenRequestDTO } from "../Models/API/DTOs/Auto/Request/createGardenRequestDTO";
import { GardenPlantResponseDTO } from "../Models/API/DTOs/Auto/Response/gardenPlantResponseDTO";
import { UserGardenResponseDTO } from "../Models/API/DTOs/Auto/Response/userGardenResponseDTO";
import GardenApiClient from "./CustomApiClients/GardenApiClient";


class GardenServiceClass {
  public async getUserGardens(): Promise<UserGardenResponseDTO[]> {
    try {
      return await GardenApiClient.getAll();
    } catch (error) {
      console.error('Error fetching user garden:', error);
      throw error;
    }
  }

  public async getUserGarden(): Promise<UserGardenResponseDTO> {
    try {
      return await GardenApiClient.customGet("");
    } catch (error) {
      console.error('Error fetching user garden:', error);
      throw error;
    }
  }

  public async createGarden(request: CreateGardenRequestDTO): Promise<UserGardenResponseDTO> {
    try {
      return await GardenApiClient.customPost('/create', request);
    } catch (error) {
      console.error('Error creating garden:', error);
      throw error;
    }
  }

  public async updateGarden(request: CreateGardenRequestDTO): Promise<UserGardenResponseDTO> {
    try {
      return await GardenApiClient.customPost('/update', request);
    } catch (error) {
      console.error('Error creating garden:', error);
      throw error;
    }
  }

  public async addPlantToGarden(request: AddPlantToGardenRequestDTO): Promise<GardenPlantResponseDTO> {
    try {
      return await GardenApiClient.customPost('/plants', request);
    } catch (error) {
      console.error('Error adding plant to garden:', error);
      throw error;
    }
  }

  public async removePlantFromGarden(gardenPlantId: number): Promise<boolean> {
    try {
      return await GardenApiClient.delete(`plants/${gardenPlantId}`);
    } catch (error) {
      console.error('Error removing plant from garden:', error);
      throw error;
    }
  }
}

export const GardenService = new GardenServiceClass();