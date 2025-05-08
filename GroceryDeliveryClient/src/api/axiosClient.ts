import axios from 'axios';

export const API_URL = 'http://127.0.0.1:5054';

const axiosClient = axios.create({
	baseURL: API_URL + '/api',
	headers: {
		'Content-Type': 'application/json',
		"Accept": "application/json"
	}
});

export default axiosClient;