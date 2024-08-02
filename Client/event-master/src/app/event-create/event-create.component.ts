import { Component } from '@angular/core';
import { EventService } from '../api';
import { CreateEventDTO,Location } from '../api';

@Component({
  selector: 'app-event-create',
  standalone: true,
  templateUrl: './event-create.component.html'
})
export class EventCreateComponent {
  event: CreateEventDTO = {
    name: null,
    description: null,
    date: '',
    location: {} as Location,
    maxParticipants: 0,
    image: null,
    categoryId: ''
  };

  constructor(private eventService: EventService) {}

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length) {
      this.event.image = input.files[0];
    }
  }

  createEvent() {
    this.eventService.createEvent(this.event).subscribe(() => {
      console.log('Event created successfully');
    });
  }
}
