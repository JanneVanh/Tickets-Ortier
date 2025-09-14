import { Show } from "./Show";

export type Reservation = {
    id: number;
    show: Show;
    numberOfAdults: number;
    numberOfChildren: number;
    paymentCode: string | null;
}