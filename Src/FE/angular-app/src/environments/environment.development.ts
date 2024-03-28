import { HttpHeaders } from "@angular/common/http";

export const environment = {
    apiUrl: 'https://localhost:7167/api',
    httpOptions: {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
        withCredentials: true,
        observe: 'response' as 'response'
    }
};
