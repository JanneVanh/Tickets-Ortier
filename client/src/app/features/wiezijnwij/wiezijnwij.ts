import { Component } from '@angular/core';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-wiezijnwij',
  imports: [
    MatIcon
  ],
  templateUrl: './wiezijnwij.html',
  styleUrl: './wiezijnwij.scss'
})
export class Wiezijnwij {
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
}
