# DDNScover

O DDNScover é um aplicativo de desktop moderno e multiplataforma desenvolvido em C# com o [Avalonia UI](https://avaloniaui.net/). Ele funciona como uma ferramenta de reconhecimento passivo, projetada para descobrir rapidamente subdomínios de um domínio alvo usando APIs públicas de OSINT (Inteligência de Fontes Abertas). Após a descoberta, o DDNScover verifica automaticamente o status online de cada subdomínio via ICMP Ping.

<img width="1920" height="1040" alt="image" src="https://github.com/user-attachments/assets/bf7250ef-8b00-4ceb-8245-90ac1b90f251" />

## Recursos

- **Descoberta Passiva de Subdomínios:** Agrega resultados simultaneamente a partir de múltiplas fontes abertas:
  - [crt.sh](https://crt.sh/) (Logs de Transparência de Certificados)
  - [AlienVault OTX](https://otx.alienvault.com/) (DNS Passivo)
  - [HackerTarget](https://hackertarget.com/) (API de busca de hosts)
- **Verificação Ativa de Hosts:** Resolve e envia automaticamente requisições de ping ICMP para os subdomínios descobertos e determina se eles estão acessíveis (`Up`) ou inacessíveis (`Down`).
- **Interface de Usuário Moderna:** Um tema escuro elegante e responsivo construído com Avalonia UI e utilizando a arquitetura MVVM (através do `CommunityToolkit.Mvvm`).
- **Exportação de Dados:** Exporte facilmente os hosts descobertos e verificados para um arquivo `.csv` diretamente na sua Área de Trabalho.
- **Multiplataforma:** Roda perfeitamente no Windows, macOS e Linux graças ao poder do .NET e Avalonia.

## Pré-requisitos

- [SDK do .NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- Uma conexão com a internet para consultar APIs OSINT externas e realizar as verificações de ping.

## Como Usar

1. Abra o DDNScover.
2. Na barra de pesquisa superior, insira seu domínio base alvo (por exemplo, `google.com` ou `exemplo.com`).
3. Clique no botão **SEARCH** (Buscar). O aplicativo buscará subdomínios a partir de várias fontes de inteligência.
4. Conforme os subdomínios preenchem a tabela de dados, o DDNScover fará o ping assincronamente neles, atualizando o indicador de status em tempo real (🟢 Ponto verde para Up/Ativo, 🔴 Ponto vermelho para Down/Inativo).
5. Após o término da pesquisa, você pode revisar os resultados. Se desejar salvar os dados, clique no botão **Export** (Exportar) para gerar um arquivo CSV na sua Área de Trabalho.

## Arquitetura e Estrutura do Projeto

- **Linguagem:** C#
- **Framework:** .NET 9.0
- **Framework de UI:** Avalonia UI (Baseado em XAML)
- **Padrão de Projeto:** MVVM (Model-View-ViewModel)

### Componentes Principais
- `Views/MainWindow.axaml`: O layout principal, estilização e configuração do data grid do aplicativo.
- `ViewModels/MainWindowViewModel.cs`: Lógica operacional principal fazendo o *binding* para a UI.
- `Services/DnsService.cs`: Orquestra a busca de dados em múltiplos provedores simultaneamente, limpando os nomes e eliminando subdomínios duplicados.
- `Services/PingService.cs`: Usa `System.Net.NetworkInformation.Ping` para realizar a verificação rápida do estado dos hosts.
- `Services/DataSources`: Implementações para cada provedor de inteligência pública individual (contrato `IDataSource`).
