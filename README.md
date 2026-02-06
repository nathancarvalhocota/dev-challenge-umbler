

# Desafio Umbler

Esta é uma aplicação web que recebe um domínio e mostra suas informações de DNS.

Este é um exemplo real de sistema que utilizamos na Umbler.

Ex: Consultar os dados de registro do dominio `umbler.com`

**Retorno:**
- Name servers (ns254.umbler.com)
- IP do registro A (177.55.66.99)
- Empresa que está hospedado (Umbler)

Essas informações são descobertas através de consultas nos servidores DNS e de WHOIS.

*Obs: WHOIS (pronuncia-se "ruís") é um protocolo específico para consultar informações de contato e DNS de domínios na internet.*

Nesta aplicação, os dados obtidos são salvos em um banco de dados, evitando uma segunda consulta desnecessaria, caso seu TTL ainda não tenha expirado.

*Obs: O TTL é um valor em um registro DNS que determina o número de segundos antes que alterações subsequentes no registro sejam efetuadas. Ou seja, usamos este valor para determinar quando uma informação está velha e deve ser renovada.*

Tecnologias Backend utilizadas:

- C#
- Asp.Net Core
- MySQL
- Entity Framework

Tecnologias Frontend utilizadas:

- Webpack
- Babel
- ES7

Para rodar o projeto você vai precisar instalar:

- dotnet Core SDK (https://www.microsoft.com/net/download/windows dotnet Core 6.0.201 SDK)
- Um editor de código, acoselhamos o Visual Studio ou VisualStudio Code. (https://code.visualstudio.com/)
- NodeJs v17.6.0 para "buildar" o FrontEnd (https://nodejs.org/en/)
- Um banco de dados MySQL (vc pode rodar localmente ou criar um site PHP gratuitamente no app da Umbler https://app.umbler.com/ que lhe oferece o banco Mysql adicionamente)

Com as ferramentas devidamente instaladas, basta executar os seguintes comandos:

Para "buildar" o javascript basta executar:

`npm install`
`npm run build`

Para Rodar o projeto:

Execute a migration no banco mysql:

`dotnet tool update --global dotnet-ef`
`dotnet tool ef database update`

E após: 

`dotnet run` (ou clique em "play" no editor do vscode)

# Objetivos:

Se você rodar o projeto e testar um domínio, verá que ele já está funcionando. Porém, queremos melhorar varios pontos deste projeto:

# FrontEnd

 - Os dados retornados não estão formatados, e devem ser apresentados de uma forma legível.
 - Não há validação no frontend permitindo que seja submetido uma requsição inválida para o servidor (por exemplo, um domínio sem extensão).
 - Está sendo utilizado "vanilla-js" para fazer a requisição para o backend, apesar de já estar configurado o webpack. O ideal seria utilizar algum framework mais moderno como ReactJs ou Blazor.  

# BackEnd

 - Não há validação no backend permitindo que uma requisição inválida prossiga, o que ocasiona exceptions (erro 500).
 - A complexidade ciclomática do controller está muito alta, o ideal seria utilizar uma arquitetura em camadas.
 - O DomainController está retornando a própria entidade de domínio por JSON, o que faz com que propriedades como Id, Ttl e UpdatedAt sejam mandadas para o cliente web desnecessariamente. Retornar uma ViewModel (DTO) neste caso seria mais aconselhado.

# Testes

 - A cobertura de testes unitários está muito baixa, e o DomainController está impossível de ser testado pois não há como "mockar" a infraestrutura.
 - O Banco de dados já está sendo "mockado" graças ao InMemoryDataBase do EntityFramework, mas as consultas ao Whois e Dns não. 

# Dica

- Este teste não tem "pegadinha", é algo pensado para ser simples. Aconselhamos a ler o código, e inclusive algumas dicas textuais deixadas nos testes unitários. 
- Há um teste unitário que está comentado, que obrigatoriamente tem que passar.
- Diferencial: criar mais testes.

# Entrega

- Enviei o link do seu repositório com o código atualizado.
- O repositório deve estar público para que possamos acessar..
- Modifique Este readme adicionando informações sobre os motivos das mudanças realizadas.

# Modificações:

## BackEnd
- Criado o DTO `DomainInfoDto` para expor apenas os dados necessários ao frontend, sem campos internos da entidade.
- Validação de domínio centralizada em `DomainNameValidator`, retornando código com mensagem descritiva e BadRequest, visando impedir que requisições inválidas cheguem ao banco de dados e evitando erro 500.
- Refatoração em camadas: controller responsável apenas por receber requisições HTTP, e delega validações e regras de domínio para `DomainService`, enquanto a persistência e consulta ao banco é responsabilidade somente de `DomainRepository`. 
- Tratamento do cenário sem registro A: status `NO_A_RECORD`, mensagem amigável e sem WHOIS completo no DTO.
- Criação de interfaces (`IDnsLookupClient`, `IWhoisClient`) para desacoplar o DomainService das libs externas e permitir mocks nos testes.
- Adapters (`DnsLookupClientAdapter`, `WhoisClientAdapter`) encapsulam o acesso real às bibliotecas e convertem a resposta para DTOs simples.
- DTOs (`DnsLookupResult`, `WhoisQueryResult`) carregam apenas os dados necessários (IP/TTL e WHOIS bruto/organização).

## Testes
- Testes reorganizados por responsabilidade (Controllers/Services/Validation).
- Criação de novos testes e melhorias nos já existentes.
 `DomainControllerTests`: Valida o payload de erro { codigo, mensagem } e respostas de sucesso.
 `DomainServiceTests`: Valida cache HIT, cache expirado, cenários sem registro A (NO_A_RECORD) e domínios inválidos.
 `DomainNameValidatorTests`: Cobre erros de falta de extensão, domínio terminando em ponto e entrada de IP.
- Testes de validação e mocks de DNS/WHOIS para evitar chamadas externas.
- Teste `Domain_Moking_WhoisClient` reativado com mocks.
- Adicionado teste para garantir que domínios inválidos retornam BadRequest.

## FrontEnd
- Resultado renderizado de forma visualmente agradável (cards/campos), sem JSON em texto bruto.
- Validação de domínio alinhada às regras do backend antes de chamar a API, visando impedir requisções malformadas pelo usuário de chegarem ao servidor.
- Botão desabilitado e indicador visual de loading durante a espera da resposta requisição.
- Modernização com React dentro do Razor/Webpack, montando o app em `#domain-app`.


- 

- 

-