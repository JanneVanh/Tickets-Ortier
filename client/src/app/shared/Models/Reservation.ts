import { Show } from "./Show";

export type Reservation = {
    id: number;
    show: Show;
    ticketsAdults: number;
    ticketsChildren: number;
}