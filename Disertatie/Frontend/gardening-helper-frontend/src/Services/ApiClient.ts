// Generic API client that can work with any entity type
export class ApiClient<T> {
  private readonly baseUrl: string;
  private readonly resourcePath: string;

  /**
   * Creates a new API client for a specific resource type
   * @param baseUrl The base URL of the API (e.g., 'https://api.example.com/api')
   * @param resourcePath The resource path (e.g., 'plants', 'users', etc.)
   */
  constructor(baseUrl: string, resourcePath: string) {
    this.baseUrl = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
    this.resourcePath = resourcePath.startsWith('/') ? resourcePath : `/${resourcePath}`;
  }

  /**
   * Get the full URL for a specific endpoint
   */
  private getUrl(path: string = ''): string {
    return `${this.baseUrl}${this.resourcePath}${path}`;
  }

  /**
   * Get all items of type T
   */
  public async getAll(): Promise<T[]> {
    try {
      const response = await fetch(this.getUrl());
      
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }
      
      return await response.json();
    } catch (error) {
      console.error(`Error fetching ${this.resourcePath}:`, error);
      throw error;
    }
  }

  /**
   * Get a single item by ID
   * @param id The unique identifier of the item
   */
  public async getById(id: number | string): Promise<T> {
    try {
      const response = await fetch(this.getUrl(`/${id}`));
      
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }
      
      return await response.json();
    } catch (error) {
      console.error(`Error fetching ${this.resourcePath}/${id}:`, error);
      throw error;
    }
  }

  /**
   * Create a new item
   * @param item The item data to create
   */
  public async create(item: Partial<T>): Promise<T> {
    try {
      const response = await fetch(this.getUrl(), {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(item),
      });
      
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }
      
      return await response.json();
    } catch (error) {
      console.error(`Error creating ${this.resourcePath}:`, error);
      throw error;
    }
  }

  /**
   * Update an existing item
   * @param id The unique identifier of the item
   * @param item The updated item data
   */
  public async update(id: number | string, item: Partial<T>): Promise<T> {
    try {
      const response = await fetch(this.getUrl(`/${id}`), {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(item),
      });
      
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }
      
      return await response.json();
    } catch (error) {
      console.error(`Error updating ${this.resourcePath}/${id}:`, error);
      throw error;
    }
  }

  /**
   * Delete an item
   * @param id The unique identifier of the item to delete
   */
  public async delete(id: number | string): Promise<boolean> {
    try {
      const response = await fetch(this.getUrl(`/${id}`), {
        method: 'DELETE',
      });
      
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }
      
      return true;
    } catch (error) {
      console.error(`Error deleting ${this.resourcePath}/${id}:`, error);
      throw error;
    }
  }

  /**
   * Perform a custom GET request to a specific endpoint
   * @param endpoint The endpoint to request (will be appended to the resource path)
   */
  public async customGet<R>(endpoint: string): Promise<R> {
    try {
      const response = await fetch(this.getUrl(endpoint));
      
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }
      
      return await response.json();
    } catch (error) {
      console.error(`Error in custom GET to ${this.resourcePath}${endpoint}:`, error);
      throw error;
    }
  }

  /**
   * Perform a custom POST request to a specific endpoint
   * @param endpoint The endpoint to request (will be appended to the resource path)
   * @param data The data to send in the request body
   */
  public async customPost<D, R>(endpoint: string, data: D): Promise<R> {
    try {
      const response = await fetch(this.getUrl(endpoint), {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      });
      
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }
      
      return await response.json();
    } catch (error) {
      console.error(`Error in custom POST to ${this.resourcePath}${endpoint}:`, error);
      throw error;
    }
  }
}

// Factory function to create API clients for different entity types
export function createApiClient<T>(resourcePath: string, baseUrl: string = 'https://localhost:7078/api'): ApiClient<T> {
  return new ApiClient<T>(baseUrl, resourcePath);
}