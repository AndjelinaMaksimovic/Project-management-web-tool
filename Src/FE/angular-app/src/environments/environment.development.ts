import { HttpHeaders } from "@angular/common/http";

export const environment = {
    apiUrl: 'http://localhost:5285/api',
    httpOptions: {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
        withCredentials: true,
        observe: 'response' as 'response'
    }
};
