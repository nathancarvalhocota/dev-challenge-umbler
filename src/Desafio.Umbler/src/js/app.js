import React from 'react'
import ReactDOM from 'react-dom'

const isIpv4Address = (value) => {
  const parts = value.split('.')
  if (parts.length !== 4) {
    return false
  }

  return parts.every((part) => {
    if (!/^\d+$/.test(part)) {
      return false
    }
    const number = Number(part)
    return number >= 0 && number <= 255
  })
}

const validateDomainInput = (value) => {
  const normalized = (value || '').trim().toLowerCase()

  if (!normalized) {
    return { isValid: false, message: 'O domínio não pode ser vazio.' }
  }

  if (normalized.length > 253) {
    return { isValid: false, message: 'O domínio excede o tamanho máximo permitido.' }
  }

  if (/\s/.test(normalized)) {
    return { isValid: false, message: 'O domínio não pode conter espaços.' }
  }

  if (normalized.includes('://') || /[\/\\?#@:]/.test(normalized)) {
    return { isValid: false, message: 'Informe apenas o domínio, sem protocolo ou caminhos.' }
  }

  if (!normalized.includes('.')) {
    return { isValid: false, message: 'O domínio deve conter uma extensão válida (ex: .com).' }
  }

  if (normalized.startsWith('.') || normalized.endsWith('.')) {
    return { isValid: false, message: 'O domínio não pode começar ou terminar com ponto.' }
  }

  if (normalized.startsWith('-') || normalized.endsWith('-')) {
    return { isValid: false, message: 'Partes do domínio não podem iniciar ou terminar com hífen.' }
  }

  if (isIpv4Address(normalized)) {
    return { isValid: false, message: 'Endereço IP não é um domínio válido.' }
  }

  const labels = normalized.split('.')
  if (labels.length < 2) {
    return { isValid: false, message: 'O domínio deve conter uma extensão válida (ex: .com).' }
  }

  for (const label of labels) {
    if (!label) {
      return { isValid: false, message: 'Partes do domínio não podem estar vazias.' }
    }

    if (label.length > 63) {
      return { isValid: false, message: 'Uma das partes do domínio excede o tamanho máximo permitido.' }
    }

    if (label.startsWith('-') || label.endsWith('-')) {
      return { isValid: false, message: 'Partes do domínio não podem iniciar ou terminar com hífen.' }
    }

    if (!/^[a-z0-9-]+$/i.test(label)) {
      return { isValid: false, message: 'Partes do domínio possuem caracteres inválidos.' }
    }
  }

  const tld = labels[labels.length - 1]
  if (/^\d+$/.test(tld)) {
    return { isValid: false, message: 'A extensão do domínio não pode ser apenas numérica.' }
  }

  return { isValid: true, value: normalized }
}

const Field = ({ label, value, fullWidth }) => {
  const className = fullWidth ? 'col-12 mb-2' : 'col-12 col-md-6 mb-2'
  return (
    <div className={className}>
      <div className="text-muted small">{label}</div>
      <div className="font-weight-bold">{value}</div>
    </div>
  )
}

const DomainResult = ({ data, fallbackName }) => {
  const nameValue = data.name || fallbackName || '-'
  const ipValue = data.ip ? data.ip : 'Sem registro A'
  const hostedAtValue = data.hostedAt ? data.hostedAt : 'Não identificado'
  const hasARecordValue = typeof data.hasARecord === 'boolean' ? (data.hasARecord ? 'Sim' : 'Não') : null

  return (
    <div className="card w-100 my-3">
      <div className="card-body">
        <h5 className="card-title mb-3">Resultado</h5>
        <div className="row">
          <Field label="Domínio" value={nameValue} />
          <Field label="IP" value={ipValue} />
          <Field label="HostedAt" value={hostedAtValue} />
          {data.status ? <Field label="Status" value={data.status} /> : null}
          {hasARecordValue ? <Field label="HasARecord" value={hasARecordValue} /> : null}
          {data.message ? <Field label="Mensagem" value={data.message} fullWidth /> : null}
        </div>
      </div>
    </div>
  )
}

const DomainApp = () => {
  const [domain, setDomain] = React.useState('')
  const [lastDomain, setLastDomain] = React.useState('')
  const [result, setResult] = React.useState(null)
  const [error, setError] = React.useState('')
  const [loading, setLoading] = React.useState(false)

  const handleSearch = async (event) => {
    event.preventDefault()
    const validation = validateDomainInput(domain)

    if (!validation.isValid) {
      setError(validation.message)
      setResult(null)
      return
    }

    setLastDomain(validation.value)
    setError('')
    setResult(null)
    setLoading(true)

    try {
      const response = await fetch(`/api/domain/${encodeURIComponent(validation.value)}`, {
        headers: { 'Accept': 'application/json' }
      })

      let payload = null
      try {
        payload = await response.json()
      } catch (parseError) {
        payload = null
      }

      if (response.status === 400) {
        setError((payload && payload.mensagem) || 'Domínio inválido.')
        return
      }

      if (!response.ok) {
        setError('Falha ao consultar o domínio.')
        return
      }

      setResult(payload)
    } catch (requestError) {
      setError('Falha ao consultar o domínio.')
    } finally {
      setLoading(false)
    }
  }

  const shouldShowResult = loading || error || result

  return (
    <div>
      <form onSubmit={handleSearch}>
        <div id="box-result" className="row">
          <div className="col-lg-9 col-md-8">
            <label className="sr-only" htmlFor="txt-search">Domínio</label>
            <input
              className="form-control form-control-lg domain"
              id="txt-search"
              name="txt-search"
              placeholder="Digite o Domínio que deseja pesquisar"
              type="text"
              value={domain}
              onChange={(event) => setDomain(event.target.value)}
            />
          </div>
          <div className="col-lg-3 col-md-4 text-xs-right">
            <button
              type="submit"
              id="btn-search"
              name="name"
              className="btn btn-success btn-block btn-lg text-xs-center"
              disabled={loading}
            >
              <i className="icon icon-search icon-white mr-1"></i>
              <span>{loading ? 'Pesquisando...' : 'Pesquisar'}</span>
            </button>
          </div>
        </div>
      </form>

      <div className="container w-100">
        <div className={`row${shouldShowResult ? '' : ' hide'}`} id="whois-results">
          {loading ? <div className="text-muted w-100">Carregando...</div> : null}
          {!loading && error ? <div className="alert alert-warning w-100">{error}</div> : null}
          {!loading && !error && result ? <DomainResult data={result} fallbackName={lastDomain} /> : null}
        </div>
      </div>
    </div>
  )
}

const root = document.getElementById('domain-app')
if (root) {
  ReactDOM.render(<DomainApp />, root)
}
