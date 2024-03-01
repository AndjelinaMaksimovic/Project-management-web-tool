# Uputstvo za pokretanje CRUD aplikacije

Tehnologija korišćena za backend je .NET (WebAPI), a za frontend Angular.

### WebAPI

WebAPI je moguće pokrenuti putem konzole korišćenjem sledeće komande: `dotnet run`

U konzoli će biti ispisan url i port za API koji je potrebno kopirati u enviroment fajl koji se nalazi u folderu **Frontend** > **src** > **evinroments** > **enviroment.ts**.

Zameniti `apiUrl` sa url-om koji je ispisan u konzoli prilikom pokretanja WebAPI servera.

```ts
export const environment = {
    apiUrl: 'https://localhost:7195'
};  
```

### Angular

Nakon podešavanja apiUrl-a moguće je pokrenuti Angular web aplikaciju.

Web aplikaciju je moguće pokrenuti putem konzole korišćenjem sledeće komande: `ng serve`