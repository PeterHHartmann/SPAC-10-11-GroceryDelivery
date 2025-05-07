import axios from 'axios';

const axiosClient = axios.create({
	baseURL: 'http://127.0.0.1:5054/api',
	headers: {
		'Content-Type': 'application/json',
		"Accept": "application/json"
	}
});

export default axiosClient;