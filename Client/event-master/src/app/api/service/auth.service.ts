import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TokenDTO } from '../model/tokenDTO';
import { UserDTO } from '../model/userDTO';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'https://api.example.com/api/auth';

  constructor(private http: HttpClient) {}

  login(user: UserDTO): Observable<TokenDTO> {
    return this.http.post<TokenDTO>(`${this.apiUrl}/login`, user);
  }

  register(user: UserDTO): Observable<TokenDTO> {
    return this.http.post<TokenDTO>(`${this.apiUrl}/register`, user);
  }

  logout(userId: string): Observable<void> {
    const headers = this.createAuthorizationHeader();
    return this.http.delete<void>(`${this.apiUrl}/logout/${userId}`, { headers });
  }

  refreshToken(refreshToken: string): Observable<TokenDTO> {
    return this.http.post<TokenDTO>(`${this.apiUrl}/refreshToken`, { refreshToken });
  }

  private createAuthorizationHeader(): HttpHeaders {
    const token = localStorage.getItem('accessToken');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  saveTokens(tokens: TokenDTO): void {
    localStorage.setItem('accessToken', tokens.accessToken);
    localStorage.setItem('refreshToken', tokens.refreshToken);
  }

  getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }
}
