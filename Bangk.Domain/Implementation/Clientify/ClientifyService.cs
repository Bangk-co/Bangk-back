using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Clientify
{
    public class ClientifyService
    {
        private readonly HttpClient _httpClient;
        private string _authToken;
        public ClientifyService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.clientify.net/")
            };
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        // Método para iniciar sesión y almacenar el token de autenticación
        public async Task LoginAsync(string endpoint, string username, string password)
        {
            var data = new
            {
                username,
                password
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                // Suponiendo que el token viene en el campo "token" en el JSON de respuesta
                var responseData = JsonConvert.DeserializeObject<dynamic>(result);
                _authToken = responseData.token;

                // Añadir el token de autorización en el encabezado para futuras solicitudes
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {_authToken}");
            }
            else
            {
                throw new Exception($"Error al autenticar: {response.StatusCode}");
            }
        }
        public async Task<string> PatchAsync<T>(string endpoint, T data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            // Crear la solicitud PATCH
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"Error en la solicitud PATCH: {response.StatusCode}");
            }
        }

        // Método genérico para solicitudes POST con token
        public async Task<string> PostAsync<T>(string endpoint, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"Error en la solicitud POST: {response.StatusCode}");
            }
        }

        // Método genérico para solicitudes GET con token
        public async Task<string> GetAsync(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"Error en la solicitud GET: {response.StatusCode}");
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
