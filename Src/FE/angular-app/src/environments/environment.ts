import { HttpHeaders } from "@angular/common/http";

export const environment = {
  apiUrl: 'http://softeng.pmf.kg.ac.rs:10122/api',
    httpOptions: {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
        withCredentials: true,
        observe: 'response' as 'response'
    }
};
