import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ReservationService } from '../../core/services/reservation';

@Component({
  selector: 'app-home',
  imports: [
    RouterLink,
],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home implements OnInit{
  private reservationService = inject(ReservationService)

  ngOnInit(): void {
    this.reservationService.emptyReservation();
  }

}
