import { Flight } from './flight';
import { FlightFilter } from './flight-filter';
import { Injectable } from '@angular/core';
import { EMPTY, Observable } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

const headers = new HttpHeaders().set('Accept', 'application/json');

@Injectable()
export class FlightService {
  flightList: Flight[] = [];
  api = 'api/flights';

  constructor(private http: HttpClient) {
  }

  findById(id: string): Observable<Flight> {
    const url = `${this.api}/${id}`;
    const params = { id: id };
    return this.http.get<Flight>(url, {params, headers});
  }

  load(filter: FlightFilter): void {
    this.find(filter).subscribe({
      next: result => {
        this.flightList = result;
      },
      error: err => {
        console.error('error loading', err);
      }
    });
  }

  find(filter: FlightFilter): Observable<Flight[]> {
    const params = {
      'from': filter.from,
      'to': filter.to,
    };

    return this.http.get<Flight[]>(this.api, {params, headers});
  }

  save(entity: Flight): Observable<Flight> {
    let params = new HttpParams();
    let url = '';
    if (entity.id) {
      url = `${this.api}/${entity.id.toString()}`;
      params = new HttpParams().set('ID', entity.id.toString());
      return this.http.put<Flight>(url, entity, {headers, params});
    } else {
      url = `${this.api}`;
      return this.http.post<Flight>(url, entity, {headers, params});
    }
  }

  delete(entity: Flight): Observable<Flight> {
    let params = new HttpParams();
    let url = '';
    if (entity.id) {
      url = `${this.api}/${entity.id.toString()}`;
      params = new HttpParams().set('ID', entity.id.toString());
      return this.http.delete<Flight>(url, {headers, params});
    }
    return EMPTY;
  }
}

