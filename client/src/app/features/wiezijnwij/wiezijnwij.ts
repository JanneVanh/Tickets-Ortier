import { Component, inject, OnInit } from '@angular/core';
import { ReservationService } from '../../core/services/reservation';

@Component({
  selector: 'app-wiezijnwij',
  imports: [
  ],
  templateUrl: './wiezijnwij.html',
  styleUrl: './wiezijnwij.scss'
})
export class Wiezijnwij implements OnInit {
  private reservationService = inject(ReservationService)

  ngOnInit(): void {
    this.reservationService.emptyReservation();
  }

  sopranen = {
    description: `Lore, Sofie, Katrien, Nancy, Anniek, Lynn, Marijke, Karlien, Zyncke, Tine, Sharon, Ann, Lieve`
  };
  alten = {
    description: `Alten in volgorde van foto`
  };
  tenoren = {
    description: `Frederik, Matty, Philippe, Cris, Henk, Pieter, Bart`
  };
  bassen = {
    description: `Bassen in volgorde van foto`
  };

  choirInfo = {
    description: `

    Simpelweg voor 't plezier van het zingen komen we al meer dan 20 jaar lang elke maandagavond samen om ... te zingen. </br>`
  };
}
