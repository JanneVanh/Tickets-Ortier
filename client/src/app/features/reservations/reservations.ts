import { Component, inject, OnInit } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { ReservationService } from '../../core/services/reservation';
import { Reservation } from '../../shared/Models/Reservation';

@Component({
  selector: 'app-reservation',
  imports: [
    MatTableModule
],
  templateUrl: './reservations.html',
  styleUrl: './reservations.scss'
})
export class Reservations implements OnInit{
  private reservationService = inject(ReservationService)
  reservations?: Reservation[];
  columnsToDisplay = ['Email', 'Show', 'Volwassenen', 'Kinderen', 'Totaal', 'Code'];

  ngOnInit(): void {
    this.reservationService.getReservations().subscribe({
      next: response => this.reservations = response,
      error: error => console.log(error)
    })
  }
}

