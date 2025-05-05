import axios, { type AxiosError, type AxiosRequestConfig, type AxiosResponse } from 'axios';

const client = axios.create({
	baseURL: 'https://127.0.0.1:7117/api',
	headers: {
		'Content-Type': 'application/json',
		"Accept": "application/json"
	}
});

export const request = async (options: AxiosRequestConfig) => {
	const onSuccess = (response: AxiosResponse) => {
		const { data } = response;
		return data;
	};

	const onError = function (error: AxiosError) {
		return Promise.reject({
			message: error.message,
			code: error.code,
			response: error.response,
		});
	};

	return client(options).then(onSuccess).catch(onError);
};

export default client;