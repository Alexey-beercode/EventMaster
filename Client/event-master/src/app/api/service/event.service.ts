import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateEventDTO, UpdateEventDTO, EventResponseDTO, EventFilterDTO } from '../model/models';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private apiUrl = 'api/event';

  constructor(private http: HttpClient) {}

  createEvent(createEventDto: CreateEventDTO): Observable<void> {
    const formData = new FormData();
    formData.append('name', createEventDto.name ?? '');
    formData.append('description', createEventDto.description ?? '');
    formData.append('date', new Date(createEventDto.date ?? '').toISOString());
    formData.append('location', JSON.stringify(createEventDto.location ?? {}));
    formData.append('maxParticipants', (createEventDto.maxParticipants ?? 0).toString());
    if (createEventDto.image) {
      formData.append('image', createEventDto.image);
    }
    formData.append('categoryId', createEventDto.categoryId ?? '');

    return this.http.post<void>(`${this.apiUrl}/create`, formData);
  }

  updateEvent(updateEventDto: UpdateEventDTO): Observable<void> {
    const formData = new FormData();
    formData.append('id', updateEventDto.id ?? '');
    formData.append('name', updateEventDto.name ?? '');
    formData.append('description', updateEventDto.description ?? '');
    formData.append('date', new Date(updateEventDto.date ?? '').toISOString());
    formData.append('location', JSON.stringify(updateEventDto.location ?? {}));
    formData.append('maxParticipants', (updateEventDto.maxParticipants ?? 0).toString());
    if (updateEventDto.image) {
      formData.append('image', updateEventDto.image);
    }
    formData.append('categoryId', updateEventDto.categoryId ?? '');

    return this.http.put<void>(`${this.apiUrl}/update`, formData);
  }

  getEvents(filter: EventFilterDTO): Observable<EventResponseDTO[]> {
    let params = new HttpParams();
    if (filter.name) params = params.append('name', filter.name);
    if (filter.date) params = params.append('date', new Date(filter.date).toISOString());
    if (filter.location) params = params.append('location', JSON.stringify(filter.location));
    if (filter.categoryId) params = params.append('categoryId', filter.categoryId);
    params = params.append('pageNumber', filter.pageNumber?.toString() ?? '1');
    params = params.append('pageSize', filter.pageSize?.toString() ?? '10');

    return this.http.get<EventResponseDTO[]>(`${this.apiUrl}/getByParams`, { params });
  }

  getEventById(id: string): Observable<EventResponseDTO> {
    return this.http.get<EventResponseDTO>(`${this.apiUrl}/getById/${id}`).pipe(
      map(event => ({
        ...event,
        imageBase64: `data:image/jpeg;base64,${event.imageBase64}`
      }))
    );
  }

  getAllEvents(): Observable<EventResponseDTO[]> {
    return this.http.get<EventResponseDTO[]>(`${this.apiUrl}/getAll`).pipe(
      map(events => events.map(event => ({
        ...event,
        imageBase64: `data:image/jpeg;base64,${event.imageBase64}`
      })))
    );
  }

  deleteEvent(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/delete/${id}`);
  }
}
