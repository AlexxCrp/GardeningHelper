// Generic API client that can work with any entity type
export class ApiClient<T> {
  private readonly baseUrl: string;
  private readonly resourcePath: string;

  constructor(baseUrl: string, resourcePath: string) {
    this.baseUrl = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
    this.resourcePath = resourcePath.startsWith('/') ? resourcePath : `/${resourcePath}`;
  }

  private getUrl(path: string = ''): string {
    return `${this.baseUrl}${this.resourcePath}${path}`;
  }

  private async getAuthHeaders(): Promise<HeadersInit> {
    const headers: HeadersInit = {
      'Content-Type': 'application/json'
    };

    const token = localStorage.getItem('token');

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    return headers;
  }

  public async getAll(): Promise<T[]> {
    try {
      const headers = await this.getAuthHeaders();
      const response = await fetch(this.getUrl(), { headers });

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`Error fetching ${this.resourcePath}:`, error);
      throw error;
    }
  }

  public async getById(id: number | string): Promise<T> {
    try {
      const headers = await this.getAuthHeaders();
      const response = await fetch(this.getUrl(`/${id}`), { headers });

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`Error fetching ${this.resourcePath}/${id}:`, error);
      throw error;
    }
  }

  public async create(item: Partial<T>): Promise<T> {
    try {
      const headers = await this.getAuthHeaders();

      const response = await fetch(this.getUrl(), {
        method: 'POST',
        headers,
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

  public async update(id: number | string, item: Partial<T>): Promise<T> {
    try {
      const headers = await this.getAuthHeaders();

      const response = await fetch(this.getUrl(`/${id}`), {
        method: 'PUT',
        headers,
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

  public async delete(id: number | string): Promise<boolean> {
    try {
      const headers = await this.getAuthHeaders();

      const response = await fetch(this.getUrl(`/${id}`), {
        method: 'DELETE',
        headers,
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

  public async customGet<R>(endpoint: string): Promise<R> {
    try {
      const headers = await this.getAuthHeaders();

      const response = await fetch(this.getUrl(endpoint), { headers });

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`Error in custom GET to ${this.resourcePath}${endpoint}:`, error);
      throw error;
    }
  }

  public async customPost<D, R>(endpoint: string, data: D): Promise<R> {
    try {
      const headers = await this.getAuthHeaders();

      const response = await fetch(this.getUrl(endpoint), {
        method: 'POST',
        headers,
        body: JSON.stringify(data),
        credentials: 'include',
      });

      if (response.status === 401) {
        throw new Error('Unauthorized - please login again');
      }

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

// Factory function
export function createApiClient<T>(resourcePath: string, baseUrl: string = 'https://localhost:7078/api'): ApiClient<T> {
  return new ApiClient<T>(baseUrl, resourcePath);
}
