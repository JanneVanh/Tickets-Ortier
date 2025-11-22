import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { Show } from '../../shared/Models/Show';
import { ShowService } from '../../core/services/showService';
import { ReservationService } from '../../core/services/reservation';

@Component({
  selector: 'app-ticketinfo',
  imports: [
    CommonModule,
    MatIcon,
    MatButton,
    RouterLink
  ],
  templateUrl: './ticketinfo.html',
  styleUrl: './ticketinfo.scss'
})
export class Ticketinfo implements OnInit {
  shows: Show[] = []
  private showService = inject(ShowService)
  private reservationService = inject(ReservationService)

  showDates = [
    { day: 'Zaterdag', date: '14 maart 2026', time: '19h30' },
    { day: 'Zondag', date: '15 maart 2026', time: '11h' },
  ];

  venue = {
    name: 'Cultuurcentrum Wevelgem',
    street: 'Acaciastraat 1',
    city: '8560 Wevelgem'
  };

  ticketPrices = [
    { type: 'Volwassene', amount: '€14' },
    { type: 'Kind (-12 jaar)', amount: '€7' }
  ];

  vipInfo = {
    description: `Een deel van de opbrengst van het concert gaat naar Tejo.be </br></br>
  TEJO staat voor "Therapeuten voor Jongeren". Dit is een initiatief dat zich inzet voor het mentaal welzijn van jongeren van 10 tot 20 jaar. 
  Ze bieden gratis en anoniem een aantal gesprekken aan bij een professionele therapeut die werkt op vrijwillige basis . `
  };

  ngOnInit(): void {
    this.reservationService.emptyReservation();
    this.showService.getShowsWithTickets().subscribe({
      next: response => this.shows = response
    })
  }

  showIsSoldOut(day: string): boolean {
    return !this.shows.some(s => s.day === day)
  }
}
