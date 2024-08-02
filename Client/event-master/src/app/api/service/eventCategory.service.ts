import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EventCategoryDTO } from '../model/eventCategoryDTO';

@Injectable({
  providedIn: 'root'
})
export class EventCategoryService {
  private readonly apiUrl = 'https://api.example.com/api/eventCategory';

  constructor(private http: HttpClient) {}

  getAll(): Observable<EventCategoryDTO[]> {
    return this.http.get<EventCategoryDTO[]>(`${this.apiUrl}/getAll`);
  }

  getById(id: string): Observable<EventCategoryDTO> {
    return this.http.get<EventCategoryDTO>(`${this.apiUrl}/getById/${id}`);
  }
}
