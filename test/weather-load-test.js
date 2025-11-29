import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    vus: 10,           
    duration: '30s',     

    thresholds: {
     
        http_req_failed: ['rate<0.01'],

   
        http_req_duration: ['p(95)<3000'],
    },
};

 
const BASE_URL = __ENV.BASE_URL || 'http://localhost:5200';

export default function () {
    const res = http.get(`${BASE_URL}/api/weather`);

    check(res, {
        'status is 200': (r) => r.status === 200,
        'duration < 3s': (r) => r.timings.duration < 3000,
    });

 
    sleep(1);
}
