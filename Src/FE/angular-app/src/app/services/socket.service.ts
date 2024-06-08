import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { HubConnectionBuilder, HubConnection, HttpTransportType } from '@microsoft/signalr';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SocketService {

  private hubConnection: HubConnection;
  private notificationSubject = new Subject<any[]>();
  ordersUpdated$: Observable<any[]> = this.notificationSubject.asObservable();

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(environment.apiUrl + '/notificationHub', {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .build();
  
    this.hubConnection
      .start()
      // .then(() => console.log('Connected to SignalR hub'))
      .catch((err: any) => console.error('Error connecting to SignalR hub:', err));
  
    this.hubConnection.on('ReceiveNotification', (notifications: any[]) => {
      console.log(notifications)
      this.notificationSubject.next(notifications);
    });
  }

}
