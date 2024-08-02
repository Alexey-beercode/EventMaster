import { Component, OnInit } from '@angular/core';
import { EventService } from '../api';
import { EventResponseDTO } from '../api';

@Component({
  selector: 'app-event-list',
  standalone: true,
  templateUrl: './event-list.component.html'
})
export class EventListComponent implements OnInit {
  events: EventResponseDTO[] = [];

  constructor(private eventService: EventService) {}

  ngOnInit() {
    this.eventService.getAllEvents().subscribe(events => {
      this.events = events;
    });
  }
}
