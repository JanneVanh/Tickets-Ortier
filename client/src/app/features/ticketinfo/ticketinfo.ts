import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

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
export class Ticketinfo {
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
    { type: 'volwassene', amount: '€14' },
    { type: 'kind (-12)', amount: '€7' }
  ];

  vipInfo = {
    description: `Een deel van de opbrengst van het concert gaat naar Tejo.be </br></br>
  Lorem ipsum dolor sit amet consectetur adipiscing elit. 
  Quisque faucibus ex sapien vitae pellentesque sem placerat. 
  In id cursus mi pretium tellus duis convallis. 
  Tempus leo eu aenean sed diam urna tempor. 
  Pulvinar vivamus fringilla lacus nec metus bibendum egestas. 
  Iaculis massa nisl malesuada lacinia integer nunc posuere. 
  Ut hendrerit semper vel class aptent taciti sociosqu. 
  Ad litora torquent per conubia nostra inceptos himenaeos.</br></br>
  Lorem ipsum dolor sit amet consectetur adipiscing elit. 
  Quisque faucibus ex sapien vitae pellentesque sem placerat. 
  In id cursus mi pretium tellus duis convallis. 
  Tempus leo eu aenean sed diam urna tempor. 
  Pulvinar vivamus fringilla lacus nec metus bibendum egestas. 
  Iaculis massa nisl malesuada lacinia integer nunc posuere. 
  Ut hendrerit semper vel class aptent taciti sociosqu. 
  Ad litora torquent per conubia nostra inceptos himenaeos.`
  };
}
