import { Component } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [
    MatButton,
    MatIcon,
    RouterLink,
    RouterLinkActive
],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home {

}
